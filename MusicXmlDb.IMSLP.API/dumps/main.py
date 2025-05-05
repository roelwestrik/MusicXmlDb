import os
import requests
import json
import argparse
import logging
from datetime import datetime

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

def fetch_api_data(data_folder, api_url, stop_after=None):
    """Fetch paginated API data for composers or works and store each page separately."""
    start = 0
    pages_fetched = 0
    step_size = 1000
    more_results = True
    data = {}

    try:
        while more_results:
            json_filepath = os.path.join(data_folder, f"page_{start}_{start + step_size - 1}.json")

            if os.path.exists(json_filepath):
                logger.info(f"Skipping page {pages_fetched}, file already exists.")
            else:
                url = api_url.replace("PAGE", str(start))  
                logger.info(f"Fetching data from: {url}")

                response = requests.get(url)
                response.raise_for_status()
                data = response.json()

                with open(json_filepath, "w", encoding="utf-8") as f:
                    json.dump(data, f, indent=4, ensure_ascii=False)
                logger.info(f"Saved page {start} to {json_filepath}")

                more_results = data.get("metadata", {}).get("moreresultsavailable", False)

            pages_fetched += 1
            if stop_after is not None and pages_fetched >= stop_after:
                logger.info(f"Reached stop_after limit: {stop_after} pages.")
                break  

            if not more_results:
                logger.info("No more results available, stopping pagination.")
                break  

            start += step_size

    except requests.exceptions.RequestException as e:
        logger.error(f"Error fetching data: {e}")
        raise

if __name__ == "__main__":
    try:
        # Only load dotenv if necessary
        from dotenv import load_dotenv, find_dotenv
        
        # Load environment variables from .env file
        env_file = find_dotenv(raise_error_if_not_found=True, usecwd=True)
        load_dotenv(env_file, override=True)

        # Get API URLs from environment variables
        API_URL = os.getenv("API_URL")
        if not API_URL:
            raise ValueError("URL not specified: make sure API_URL is set in the .env file!")
            
        parser = argparse.ArgumentParser(description="Fetch paginated API data and store each page separately")
        parser.add_argument(
            "--api_types",
            type=str,
            choices=["composers", "works"],
            nargs="+",  # Allows multiple values (e.g., `--api_types composers works`)
            required=True,
            help="API type(s) to fetch (space-separated: 'composers works')"
        )
        parser.add_argument("--output_folder", type=str, help="Custom output folder for storing JSON pages")
        parser.add_argument("--stop_after", type=int, help="Maximum number of pages to fetch (optional)")
        args = parser.parse_args()

        for api_type in args.api_types:
            # Select the correct API URL based on the user's choice
            api_url = API_URL.replace("TYPE", "1") if api_type == "composers" else API_URL.replace("TYPE", "2")
            
            today = datetime.today().strftime("%Y-%m-%d")
            base_folder = args.output_folder if args.output_folder else "data"
            data_folder = os.path.join(base_folder, api_type, today)  # Separate "composers" and "works"
            os.makedirs(data_folder, exist_ok=True)

            fetch_api_data(data_folder, api_url, args.stop_after)

    except Exception as e:
        print(f"An unexpected error occurred: {e}")
