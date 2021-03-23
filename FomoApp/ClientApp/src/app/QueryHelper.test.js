import QueryHelper from './QueryHelper'

beforeEach(() => {

});

afterEach(() => {
});

it("CreateIdsQuery should create empty query string when no ids", () => {

    const queryString = QueryHelper.createIdsQuery("someKey", []);

    expect(queryString).toEqual("");
});

it("CreateIdsQuery should create empty query string when no key", () => {

    const queryString = QueryHelper.createIdsQuery(null, [10]);

    expect(queryString).toEqual("");
});


it("CreateIdsQuery should create query string for one id", () => {
    const queryString = QueryHelper.createIdsQuery("someKey", [10]);

    expect(queryString).toEqual("someKey=10");

});

it("CreateIdsQuery should create query string for multiple ids", () => {
    const queryString = QueryHelper.createIdsQuery("someKey", [10, 20, 30]);

    expect(queryString).toEqual("someKey=10&someKey=20&someKey=30");
});