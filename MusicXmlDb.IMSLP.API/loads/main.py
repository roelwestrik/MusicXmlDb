import json
import argparse
import os
from datetime import datetime
import logging

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

def load_diff_file(diff_folder, api_type, diff_file):
    """Loads the JSON diff file."""
    diff_path = os.path.abspath(os.path.join(os.getcwd(), diff_folder, api_type, diff_file))
    if not os.path.exists(diff_path):
        raise FileNotFoundError(f"Diff file not found: {diff_path}")

    with open(diff_path, "r", encoding="utf-8") as f:
        return json.load(f)

def generate_sql_queries_work(diff_data):
    """Generates SQL UPDATE queries based on the diff data."""
    queries = []
    
    if "$replace" in diff_data:
        for id, entry in diff_data["$replace"].items():
            if id == "metadata":
                continue

            work_id = entry["id"]
            composer = entry["intvals"]["composer"]
            worktitle = entry["intvals"]["worktitle"]

            query = f"""
            UPDATE works 
            SET 
                composer = '{composer}',
                worktitle = '{worktitle}',
            WHERE pageid = '{work_id}';
            """
            queries.append(query.strip())

    return queries

def generate_sql_queries_composer(diff_data):
    """Generates SQL UPDATE queries based on the diff data."""
    queries = []
    
    if "$replace" in diff_data:
        for id, entry in diff_data["$replace"].items():
            if id == "metadata":
                continue

            composer_id = entry["id"]
            imslp_link = entry["permlink"]

            query = f"""
            UPDATE composers 
            SET 
                imslp_link = '{imslp_link}'
            WHERE composerid = '{composer_id}';
            """
            queries.append(query.strip())

    return queries

def generate_sql_filename(diff_file, output_folder):
    """Generates an SQL filename based on the diff file name and timestamp."""
    diff_name = os.path.basename(diff_file).replace(".json", "")
    timestamp = datetime.now().strftime("%Y%m%d-%H%M%S")
    
    filename = f"sql_{diff_name}_{timestamp}.sql"
    return os.path.join(output_folder, filename)

def save_sql_queries(queries, diff_file, output_folder):
    """Saves SQL queries to a file in the specified folder."""
    if not os.path.exists(output_folder):
        os.makedirs(output_folder)

    sql_filepath = generate_sql_filename(diff_file, output_folder)

    with open(sql_filepath, "w", encoding="utf-8") as f:
        f.write("\n".join(queries))

    logger.info(f"SQL queries saved to {sql_filepath}")

def main():
    try:
        parser = argparse.ArgumentParser(description="Generate SQL queries from JSON diff")
        parser.add_argument("--diff_folder", type=str, required=True, help="Folder where JSON diff results are stored")
        parser.add_argument("--diff_file", type=str, required=True, help="Filename JSON diff file")
        parser.add_argument("--output_folder", type=str, required=True, help="Folder to store SQL output")
        parser.add_argument(
            "--api_types",
            type=str,
            choices=["composers", "works"],
            nargs="+",  # Allows multiple values (e.g., `--api_types composers works`)
            required=True,
            help="API type(s) to fetch (space-separated: 'composers works')"
        )
        args = parser.parse_args()

        for api_type in args.api_types:
            diff_data = load_diff_file(args.diff_folder, api_type, args.diff_file)
            sql_queries = (
                generate_sql_queries_composer(diff_data) if api_type == "composers" else 
                generate_sql_queries_work(diff_data)
            )

            output_folder = os.path.abspath(os.path.join(os.getcwd(), args.output_folder, api_type))
            save_sql_queries(sql_queries, args.diff_file, output_folder)
    except Exception as e:
        logger.exception(f"Error: {e}")
        raise

if __name__ == "__main__":
    try:
        main()
    except Exception as e:
        print(f"Error: {e}")
