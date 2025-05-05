import os
import requests
import logging
from datetime import datetime

from musicxmldb_imslp_api.models.composer import Composer
from musicxmldb_imslp_api.models.meta_data import MetaData
from musicxmldb_imslp_api.models.work import Work

import argparse

# Argument parsing at the top
parser = argparse.ArgumentParser(description="Run my_project with optional output_folder.")
parser.add_argument("--output_folder", type=str, help="Path to the output folder", default=None)
args = parser.parse_args()

# Set up logging
logging.basicConfig(
    filename="logs.log",
    level=logging.INFO,
    format="%(asctime)s - %(levelname)s - %(name)s -  %(message)s"
)
console_handler = logging.StreamHandler()
console_handler.setLevel(logging.INFO)
logger = logging.getLogger(__name__)
logger.addHandler(console_handler)

def construct_url(base_url, account, disclaimer, sort, type, start, retformat):
    url = f"{base_url}?account={account}/disclaimer={disclaimer}/sort={sort}/type={type}/start={start}/retformat={retformat}"
    return url

def parse_metadata(json): 
    metadata = MetaData(json)
    return metadata

def parse_composer(json):
    composer = Composer(json)
    return composer

def parse_work(json):
    work = Work(json)
    return work

def parse_composers_json(json):
    composers = []
    metadata = None

    for k, v in json.items():
        if k == "metadata":
            metadata = parse_metadata(v)
        if k.isnumeric():
            composers.append(parse_composer(v))

    return composers, metadata

def parse_works_json(json):
    works = []
    metadata = None

    for k, v in json.items():
        if k == "metadata":
            metadata = parse_metadata(v)
        if k.isnumeric():
            works.append(parse_work(v))

    return works, metadata

def to_composer_bulk_insert(composers):
    values = ", ".join(f"('{composer.id}', FALSE)" for composer in composers)
    bulk_upsert_sql = f"""
INSERT INTO composer (name, pending_deletion)
VALUES {values}
ON CONFLICT (name) DO UPDATE SET pending_deletion = FALSE;
"""
    return bulk_upsert_sql

def to_work_bulk_insert(works):
    values = ", ".join(f"('{work.id}', FALSE)" for work in works)
    bulk_upsert_sql = f"""
INSERT INTO composer (name, pending_deletion)
VALUES {values}
ON CONFLICT (name) DO UPDATE SET pending_deletion = FALSE;
"""
    return bulk_upsert_sql

def get_composers(max_pages=1):
    start = 0
    pages = 0
    has_more_data = True
    composers = []

    while has_more_data & pages < max_pages:
        url = construct_url(
            base_url="http://imslp.org/imslpscripts/API.ISCR.php",
            account="worklist",
            disclaimer="accepted",
            sort="id",
            type="1",
            start=start,
            retformat="json"
        )
        logger.info(f"Getting data from {url}")
        response = requests.get(url)
        json = response.json()

        _composers, metadata = parse_composers_json(json)
        composers += _composers

        has_more_data = metadata.moreresultsavailable
        start += metadata.limit
        pages += 1

    return composers

def get_works(max_pages=1):
    start = 0
    pages = 0
    has_more_data = True
    works = []

    while has_more_data & pages < max_pages:
        url = construct_url(
            base_url="http://imslp.org/imslpscripts/API.ISCR.php",
            account="worklist",
            disclaimer="accepted",
            sort="id",
            type="2",
            start=start,
            retformat="json"
        )
        logger.info(f"Getting data from {url}")
        response = requests.get(url)
        json = response.json()

        _works, metadata = parse_works_json(json)
        works += _works

        has_more_data = metadata.moreresultsavailable
        start += metadata.limit
        pages += 1
    
    return works

def main(output_folder):
    """Fetch paginated API data for composers or works and store each page separately."""
    try:
        timestamp = datetime.now().strftime("%Y%m%d_%H%M")
        filename = f"bulk_update_{timestamp}.sql"
        filedir = os.path.abspath(os.path.join(os.getcwd(), output_folder))
        os.makedirs(filedir, exist_ok=True)

        filepath = os.path.join(filedir, filename)
        logger.info(f"Creating file {filepath}.")

        with open(filepath, "w", encoding="utf-8") as sql_file:

            sql_file.write("BEGIN;")
            sql_file.write(os.linesep)
            sql_file.write(
"""
CREATE TYPE IF NOT EXISTS IntVals AS (
    composer TEXT,
    worktitle TEXT,
    icatno TEXT,
    pageid INT
);
""" 
            )
            sql_file.write(os.linesep)
            sql_file.write(
"""
CREATE TABLE IF NOT EXISTS Composer (
    id TEXT PRIMARY KEY,
    type TEXT NOT NULL,
    parent TEXT,
    permlink TEXT NOT NULL,
    intvals IntVals NULL  -- Optional field
);
"""
            )
            sql_file.write(os.linesep)
            sql_file.write(
"""
CREATE TABLE IF NOT EXISTS Work (
    id TEXT PRIMARY KEY,
    type TEXT NOT NULL,
    parent TEXT NOT NULL REFERENCES Composer(id) ON DELETE CASCADE,
    permlink TEXT NOT NULL,
    intvals IntVals NULL  -- Optional field
);
"""
            )
            sql_file.write(os.linesep)

            sql_file.write(
"""
ALTER TABLE Composer ADD COLUMN IF NOT EXISTS pending_deletion BOOLEAN DEFAULT FALSE;
ALTER TABLE Work ADD COLUMN IF NOT EXISTS pending_deletion BOOLEAN DEFAULT FALSE;
"""
            )
            sql_file.write(os.linesep)

            logger.info("Getting composers.")
            composers = get_composers()
            bulk_upsert_sql = to_composer_bulk_insert(composers) 
            sql_file.write(bulk_upsert_sql)
            sql_file.write(os.linesep)

            logger.info("Getting works.")
            works = get_works()
            bulk_upsert_sql = to_work_bulk_insert(works) 
            sql_file.write(bulk_upsert_sql)
            sql_file.write(os.linesep)

            sql_file.write("COMMIT;")
            sql_file.write(os.linesep)

        logger.info("File closed succesfully.")

    except requests.exceptions.RequestException as e:
        logger.error(f"Error fetching data: {e}")
        raise
    finally:
        logger.info("Exiting application now.")

if __name__ == "__main__":
    try:
        output_folder = args.output_folder
        main(output_folder=output_folder)
    except Exception as e:
        print(f"An unexpected error occurred: {e}")
