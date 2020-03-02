import { ACCOUNTS_API_URL, BASE_URL } from '../common/constants';
import { sendFetch } from '../services/ApiClient';

export const UNAUTHORIZED_STATUS = 'UNAUTHORIZED_RESPONSE';
export const FAILED_LOGIN = 'FAILED_LOGIN';
const UNAUTHORIZED_STATUS_CODE = 401;

export function unauthorizedResponse() {
    return {
        type: UNAUTHORIZED_STATUS
    };
}

export function failedLogin(loginText) {
	return {
		type: FAILED_LOGIN,
		text: loginText
	};
}

export function checkLoginThunk() {
	return function (dispatch) {
		return fetch(`${ACCOUNTS_API_URL}/CheckLogin`, { credentials: "include" })
			.then(function (response) {
				if (response.status === UNAUTHORIZED_STATUS_CODE) {
					dispatch(unauthorizedResponse());
				}
				else if (!response.ok){
					dispatch(failedLogin(response.statusText));
				}
			})
			.catch(function (response) {
				dispatch(failedLogin(response.statusText));
			});
	};
}