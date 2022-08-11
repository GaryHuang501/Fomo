import './ProfileSettings.css';

import React, { useState } from 'react';

/*
    Form to edit the users profile
*/
export default function ProfileSettings(props){

    const minNameLength = process.env.REACT_APP_MIN_NAME_LENGTH;
    const maxNameLength = process.env.REACT_APP_MAX_NAME_LENGTH;

    const myUser = props.myUser ?? {};
    const [newUserName, setNewUserName] = useState('');
    const [clientErrors, setClientErrors] = useState('');
    const apiErrors = props.apiErrors;
    
    function handleNameChange(e){
        const newUserName = e.target.value;
        setNewUserName(newUserName);
    }

    function getClientFormErrors(){
        if(!newUserName || newUserName.length < minNameLength || newUserName.length > maxNameLength ){
            return `Username must be within ${minNameLength} and ${maxNameLength} characters.`;
        }
        
        return null;
    }

    function formatApiError(apiError){
        if (typeof apiError === 'string'){
            return apiError;
        }
        else if(apiError && 'description' in apiError)
        {
            return apiError.description;
        }
        else{
            return "Unexpected Error";
        }
    }

    function onSubmit(e){
        e.preventDefault();

        const clientFormErrors = getClientFormErrors();

        if(clientFormErrors){
            setClientErrors(clientFormErrors);
            return;
        }

        const user = {
            ...myUser,
            name: newUserName
        };
        
        if(props.onSubmit){
            props.onSubmit(user);
        }
    }

    return (    
        <div id='user-profile-settings'>
            <h3>{props.heading}</h3>
            <div id='user-profile-settings-input-container'>
                <input type='text' id="input-user-profile-settings-name" className='form-input' placeholder='Set User Name'name="input-user-profile-settings-name" value={newUserName} onChange={handleNameChange}/><br></br>
            </div>
            <input id='user-profile-settings-submit' className='submit-input' type="submit" value="Submit" onClick={onSubmit}></input>
            { clientErrors ? <p className='input-user-profile-settings-name-error'> {clientErrors}</p> : <br></br>}
            { apiErrors ? <p className='input-user-profile-settings-name-error'> {formatApiError(apiErrors)}</p> : <br></br>}
        </div>
    );
}