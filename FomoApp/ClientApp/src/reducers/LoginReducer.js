import * as LoginActions from '../actions/LoginActions';

export function loginReducer(state, action) {

    if (state === undefined) {
        return {
            isUnauthorized: false
        };
    }

    switch (action.type) {
		case LoginActions.UNAUTHORIZED_STATUS:
            return Object.assign({}, {
                isUnauthorized: true
			});
		case LoginActions.FAILED_LOGIN:
			return Object.assign({}, {
				isFailedLogin: true
			});
        default:
            return state;
    }
}

