import { connect } from 'react-redux';
import { Login } from '../components/Login';
import { checkLoginThunk } from '../actions/LoginActions';

export function mapStateToProps(state) {
    return {
		isUnauthorized: state.loginReducer.isUnauthorized,
		isFailedLogin: state.loginReducer.isFailedLogin
    };
}

export function mapDispatchToProps(dispatch) {
	return {
		checkLogin: () => dispatch(checkLoginThunk())
	};
}

export const LoginContainer = connect(
	mapStateToProps,
	mapDispatchToProps
)(Login);
