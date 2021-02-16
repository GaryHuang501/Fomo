import 'firebase/auth';
import 'firebase/database';

import {getClientCustomToken, selectClientCustomToken, selectFirebaseAuthenticatedState, setFireBaseAuthenticated} from  '../features/login/LoginSlice';
import { useDispatch, useSelector } from 'react-redux';

import React from 'react';
import firebase from 'firebase/app';
import { useEffect } from 'react/cjs/react.development';

// Manages the login for firebase
export const FirebaseManager = function() {

  const dispatch = useDispatch();
  const clientToken = useSelector(selectClientCustomToken);

  useEffect(() => {
    dispatch(getClientCustomToken());
  }, [dispatch]);

  useEffect(async () => {
    // sign with custom token once fetched.
    if(clientToken){
      try{
        await firebase.auth().signInWithCustomToken(clientToken)
        dispatch(setFireBaseAuthenticated());
      }
      catch(ex){
      }
    }
  }, [dispatch, clientToken]);


  return null;
}


