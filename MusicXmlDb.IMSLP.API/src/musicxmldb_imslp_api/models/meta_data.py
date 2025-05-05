class MetaData:
    start: int
    limit: int
    sortby: str
    sortdirection: str
    moreresultsavailable: bool
    timestamp: int
    apiversion: int

    def __init__(self, dict):
        self.__dict__.update(dict)