import { unauthorizedResponse } from '../actions/LoginActions';

const UNAUTHORIZED_STATUS = 401;

export function generateQueryString(queryValues = {}) {
    var totalKeyCount = Object.keys(queryValues).length;

    if (totalKeyCount === 0) return '';
    let queryString = '?';
    let currentKeyIndex = 0;
    for (var key in queryValues) {
        if (queryValues.hasOwnProperty(key)) {
            queryString += key + '=' + queryValues[key];

            if (currentKeyIndex < totalKeyCount - 1)
                queryString += '&';
        }
        currentKeyIndex++;
    }
    return queryString;
}

export function sendFetch(url, dispatch, queryValues) {
    var queryString = generateQueryString(queryValues);
    return fetch(url + queryString)
        .then(function (response) {
            if (response.status === UNAUTHORIZED_STATUS) {
                dispatch(unauthorizedResponse());
            }

            if (!response.ok) {
                throw Error(response.statusText);
            }

            return response.json();
        });
}

