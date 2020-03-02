import React, { Component } from 'react';
import './Login.css';
import { ACCOUNTS_API_URL, BASE_URL } from '../common/constants';

export class Login extends Component {

	componentDidMount() {
		this.props.checkLogin();
	}

	signIn(provider) {
		window.location.href = `${ACCOUNTS_API_URL}/login?provider=${provider}&returnUrl=${BASE_URL}`;
	}

    render() {


		if (this.props.isUnauthorized || this.props.isFailedLogin) {

			var loginErrorMessage = this.props.isFailedLogin ? "There was an error logging. Try again." : "";

            return (
                <div>
                    <div id="login-overlay" />
					<div id="login-modal" className='layout-border'>
                        <div id='sign-in-container' >
                            <p id='sign-in-header'>Please Login</p>
                            <div className='sign-in-button-list'>
                                <div className='google-sign-in-button sign-in-button'
                                    onClick={() => this.signIn('Google')}
                                />
                                <p> OR </p>
                                <div id="sign-in-test-button" className='sign-in-button layout-border'> Use Test Account</div>
							</div>
							<p id="login-modal-error-message">{loginErrorMessage}</p>
                        </div>
                    </div>
                </div>
            );
		}
        else {
            return null;
        }
    }
}
