from musicxmldb_imslp_api.models.int_vals import IntVals


class Work:
    id: str
    type: str
    parent: str
    intvals: "IntVals"
    permlink: str

    def __init__(self, dict):
        self.__dict__.update(dict)
        self.intvals = IntVals(dict.get("intvals", {}))