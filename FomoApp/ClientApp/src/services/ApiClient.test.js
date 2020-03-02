import { UNAUTHORIZED_STATUS, unauthorizedResponse } from '../actions/LoginActions';
import { sendFetch } from './ApiClient';
import { fetchMock, MATCHED, UNMATCHED, Response} from 'fetch-mock';

var mockDispatch;

beforeEach(() => {

    mockDispatch = () => { };
});

afterEach(() => {
    fetchMock.reset();
});

test("should make a fetch call to the correct url with query parameters", () => {
    const url = "https://test.com";
    var expectedData = { data: "123" };
    fetchMock.get(url, expectedData);
    return sendFetch(url, mockDispatch)
        .then(function (response) {
            expect(response).toEqual(expectedData);
        });
});

test("should make a fetch call to the correct url and query parameters", () => {
    const url = "https://test.com";
    var params = { flag1: "1", flag2: "2" };
    var expectedData = { data: "123" };
    fetchMock.get(url + "?flag1=1&flag2=2", expectedData);
    fetchMock.get(url + "?flag2=2&flag1=1", expectedData);

    return sendFetch(url, mockDispatch, params)
        .then(function (response) {
            expect(response).toEqual(expectedData);
        });
});

test("should dispatch unauthorized_event when 401 received", () => {
    const url = "https://test.com";
    let isUauthorizedResponse = false;
    fetchMock.get(url, 401);
    mockDispatch = () => { isUauthorizedResponse = true;};
    return sendFetch(url, mockDispatch).catch(function (error)
        {
            expect(isUauthorizedResponse).toBeTruthy();
        });
});

test("should throw an error when response is not 200 OK", () => {
    const url = "https://test.com";
    fetchMock.get(url, 500);
    let isError = false;
    return sendFetch(url, mockDispatch)
        .catch(function (error) {
            isError = true;
        })
        .then(function (error) {
            expect(isError).toBeTruthy();
        });
});

