import './Login.css';

import React from 'react';
import googleButton from '../../assets/login/btn_google_signin_dark_normal_web.png'

/*
    Modal for logging in. Automatically appears when user is uanauthenicated during web page laod.

    Remarks: HTML is declared here instead of using modal.js since this modal is loaded before the dom tree has fully built.
    ModalRoot is undefined.
*/
export const LoginModal = () => {

    function signIn(e){
        const provider = e.currentTarget.dataset.column;
        window.location.href = `${process.env.REACT_APP_API_URL}/accounts/login?provider=${provider}&returnurl=${window.location.href}`;
    }

    return (
        <div id="login-modal" className='modal-backdrop'>
            <form id='login-modal-form' className='modal-form'>
                <h3>Please Select a Login</h3>
                <ul>
                    <li><img className='login-button login-google-button' src={googleButton} alt='Google Sign in' onClick={signIn} data-column='Google' role='button'></img></li>
                    <li><div className='login-button login-test-button' onClick={signIn} data-column='Test'>Use Test Account</div></li>
                </ul>             
            </form>
        </div>
    )
}