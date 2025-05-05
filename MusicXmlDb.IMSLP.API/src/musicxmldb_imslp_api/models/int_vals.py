class IntVals:
    composer: str
    worktitle: str
    icatno: str
    pageid: int

    def __init__(self, dict):
        self.__dict__.update(dict)