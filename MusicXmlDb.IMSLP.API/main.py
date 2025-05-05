import os
import requests
import json
import argparse
import logging
from datetime import datetime

# Set up logging
logging.basicConfig(
    filename="api_scraper.log",
    level=logging.INFO,
    format="%(asctime)s - %(levelname)s - %(message)s"
)
console_handler = logging.StreamHandler()
console_handler.setLevel(logging.INFO)
logging.getLogger().addHandler(console_handler)

def fetch_api_data(data_folder, api_url, stop_after=None):
    """Fetch paginated API data for composers or works and store each page separately."""
    start = 0
    pages_fetched = 0
    more_results = True
    data = {}

    try:
        logging.info(f"Starting API fetch from {api_url}...")

        while more_results:
            json_filepath = os.path.join(data_folder, f"page_{start}.json")

            if os.path.exists(json_filepath):
                logging.info(f"Skipping page {start}, file already exists.")
            else:
                url = api_url.replace("PAGE", str(start))  
                logging.info(f"Fetching data from: {url}")

                response = requests.get(url)
                response.raise_for_status()
                data = response.json()

                with open(json_filepath, "w", encoding="utf-8") as f:
                    json.dump(data, f, indent=4, ensure_ascii=False)
                logging.info(f"✅ Saved page {start} to {json_filepath}")

                more_results = data.get("metadata", {}).get("moreresultsavailable", False)

            pages_fetched += 1
            if stop_after is not None and pages_fetched >= stop_after:
                logging.info(f"Reached stop_after limit: {stop_after} pages.")
                break  

            if not more_results:
                logging.info("No more results available, stopping pagination.")
                break  

            start += 1000

    except requests.exceptions.RequestException as e:
        logging.error(f"❌ Error fetching data: {e}")

if __name__ == "__main__":
    try:
        # Only load dotenv if necessary
        from dotenv import load_dotenv
        
        # Load environment variables from .env file
        load_dotenv()

        # Get API URLs from environment variables
        API_COMPOSERS = os.getenv("API_COMPOSERS")
        API_WORKS = os.getenv("API_WORKS")

        parser = argparse.ArgumentParser(description="Fetch paginated API data and store each page separately")
        parser.add_argument("--api_type", type=str, choices=["composers", "works"], required=True, help="API type to fetch (composers or works)")
        parser.add_argument("--output_folder", type=str, help="Custom output folder for storing JSON pages")
        parser.add_argument("--stop_after", type=int, help="Maximum number of pages to fetch (optional)")
        args = parser.parse_args()

        # Select the correct API URL based on the user's choice
        api_type = args.api_type == "composers"
        api_url = API_COMPOSERS if api_type else API_WORKS
        if not api_url:
            raise ValueError(f"❌ URL not specified: make sure {"API_COMPOSERS" if api_type == "composeres" else "API_WORKS"} is set in the .env file!")
        
        today = datetime.today().strftime("%Y-%m-%d")
        base_folder = args.output_folder if args.output_folder else os.path.join("data", today)
        data_folder = os.path.join(base_folder, api_type)  # Separate "composers" and "works"
        os.makedirs(data_folder, exist_ok=True)

        fetch_api_data(data_folder, api_url, args.stop_after)

    except Exception as e:
        logging.error(f"❌ Unexpected error: {e}")
        print(f"❌ An unexpected error occurred: {e}")
