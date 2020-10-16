import React, { useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux'
import googleButton from '../../assets/login/btn_google_signin_dark_normal_web.png'
import './Login.css';
import { checkLogin, setAuthenticated } from './LoginSlice';
import config from '../../app/Config.json'

export const LoginModal = () => {

    const dispatch = useDispatch();
    const apiUrl = config.apiUrl;

    function signIn(provider){
        window.location.href = `${apiUrl}/accounts/login?provider=${provider}&returnurl=${window.location.href}`;
    }

    return (
        <div id="login-modal" >
            <form id='login-modal-form'>
                <h3>Please Select a Login</h3>
                <ul>
                    <li><img className='login-button login-google-button' src={googleButton} onClick={() => signIn("Google")}></img></li>
                    <li><div className='login-button login-test-button' onClick={() => signIn("Test")}>Use Test Account</div></li>
                </ul>             
            </form>
        </div>
    )
}