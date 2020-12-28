import React from 'react';
import googleButton from '../../assets/login/btn_google_signin_dark_normal_web.png'
import './Login.css';

export const LoginModal = () => {

    const apiUrl = process.env.REACT_APP_API_URL;

    function signIn(e){
        const provider = e.currentTarget.dataset.column;
        window.location.href = `${apiUrl}/accounts/login?provider=${provider}&returnurl=${window.location.href}`;
    }

    return (
        <div id="login-modal" className='modal-backdrop'>
            <form id='login-modal-form'>
                <h3>Please Select a Login</h3>
                <ul>
                    <li><img className='login-button login-google-button' src={googleButton} alt='Google Sign in' onClick={signIn} data-column='Google'></img></li>
                    <li><div className='login-button login-test-button' onClick={signIn} data-column='Test'>Use Test Account</div></li>
                </ul>             
            </form>
        </div>
    )
}