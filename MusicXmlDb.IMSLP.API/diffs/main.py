import os
import json
import argparse
import logging
import jsondiff
from datetime import datetime

# Set up logging
logging.basicConfig(
    filename="logs.log",
    level=logging.INFO,
    format="%(asctime)s - %(levelname)s - %(filename)s -  %(message)s"
)
console_handler = logging.StreamHandler()
console_handler.setLevel(logging.INFO)
logger = logging.getLogger(__file__)
logger.addHandler(console_handler)

def load_json_files(folder_path):
    """Loads and concatenates JSON files in a folder into a single dictionary."""
    combined_data = {}

    if not os.path.exists(folder_path):
        logger.error(f"Folder does not exist: {folder_path}")
        return combined_data

    for filename in os.listdir(folder_path):
        if filename.endswith(".json"):
            file_path = os.path.join(folder_path, filename)
            try:
                with open(file_path, "r", encoding="utf-8") as f:
                    data = json.load(f)
                    combined_data.update(data)  # Merge JSON objects
                    logging.info(f"✅ Loaded: {filename}")
            except json.JSONDecodeError:
                logger.error(f"Error decoding JSON: {filename}")
    
    return combined_data

def resolve_path(directory, *paths):
    """Resolve the path relative to the current working directory if it's not absolute."""
    return os.path.abspath(os.path.join(os.getcwd(), directory, *paths))  # Ensure it's relative to cwd

def generate_diff_filename(folder1, folder2, diff_folder):
    """Generates a filename that includes dataset names and a timestamp."""
    dataset1_name = os.path.basename(folder1) if folder1 else "empty"
    dataset2_name = os.path.basename(folder2)
    
    filename = f"diff_{dataset1_name}_vs_{dataset2_name}.json"
    return os.path.join(diff_folder, filename)

def save_json_diff(diff_data, folder1, folder2, diff_folder):
    """Saves JSON differences to a named file, converting keys to strings."""
    if not os.path.exists(diff_folder):
        os.makedirs(diff_folder)

    diff_filepath = generate_diff_filename(folder1, folder2, diff_folder)

    # Convert diff keys to strings
    diff_serializable = {str(k): v for k, v in diff_data.items()}

    with open(diff_filepath, "w", encoding="utf-8") as f:
        json.dump(diff_serializable, f, indent=4, ensure_ascii=False)

    logger.info(f"JSON diff saved to {diff_filepath}")


def compare_json(data1, data2, folder1, folder2, diff_folder):
    """Compares two JSON objects and saves the differences."""
    diff = jsondiff.diff(data1, data2)

    if diff:
        logger.info("Differences found, saving to file...")
        save_json_diff(diff, folder1, folder2, diff_folder)
    else:
        logger.info("JSON objects are identical. No diff file created.")


def main(folder1, folder2, diff_folder):
    try:
        logger.info(f"Using resolved paths: {folder1}, {folder2}, Diff output: {diff_folder}")

        data1 = {} if not folder1 or not os.path.exists(folder1) else load_json_files(folder1)
        data2 = load_json_files(folder2)

        logger.info("JSON objects concatenated successfully!")

        # Compare and save JSON diff
        compare_json(data1, data2, folder1, folder2, diff_folder)
    except Exception as e:
        logger.error(f"Unexpected error: {e}")
        raise



if __name__ == "__main__":
    try:
        parser = argparse.ArgumentParser(description="Concatenate JSON files and compare differences")
        parser.add_argument("--folder1", type=str, help="Path to first folder (Optional)")
        parser.add_argument("--folder2", type=str, required=True, help="Path to second folder")
        parser.add_argument("--dump_folder", type=str, required=True, help="Folder where dumps are stored")
        parser.add_argument("--diff_folder", type=str, required=True, help="Folder to store JSON diff results")
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
            folder1 = resolve_path(args.dump_folder, api_type, args.folder1) if args.folder1 else None
            folder2 = resolve_path(args.dump_folder, api_type, args.folder2)
            diff_folder = resolve_path(args.diff_folder, api_type)

            main(folder1, folder2, diff_folder)
    except Exception as e:
        print(f"An unexpected error occurred: {e}")
