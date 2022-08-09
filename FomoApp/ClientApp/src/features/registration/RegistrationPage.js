import './Registration.css';

import React, {useState} from 'react';

import FomoLogoBanner from '../../app/FomoLogoBanner';
import ProfileSettings from '../login/ProfileSettings';
import axios from 'axios';
import { useParams } from "react-router-dom";

// Page for registering user after first time login through oauth.
export default function RegistrationPage() {

  const [errors, setErrors] = useState(null);
  const params = useParams();
  const newUser = {id: params.urlUserId}

  async function onSubmit(updatedUser) {
    
    try{
      const response = await axios.put(`${process.env.REACT_APP_API_URL}/accounts/${updatedUser.id}`, updatedUser, {withCredentials: true});
      window.location.href = window.location.origin;
    }
    catch(error){
      setErrors(error.response.data);
    }
  }

  return (
     (params == null || params.urlUserId == null) ? null :
    <React.Fragment>
      <FomoLogoBanner />
      <main id='registration-page'>
        <div id='registration-page-container' className='leader-board standard-border standard-border-radius'>
          <ProfileSettings heading="Register new User" apiErrors={errors} onSubmit={onSubmit} myUser={newUser}/>
        </div>
      </main>
    </React.Fragment>
  );
}