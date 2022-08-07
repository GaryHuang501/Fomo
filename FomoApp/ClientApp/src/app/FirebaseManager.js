import 'firebase/auth';
import 'firebase/database';

import {getClientCustomToken, selectClientCustomToken, setFirebaseAuthenticated} from  '../features/login/LoginSlice';
import { useDispatch, useSelector } from 'react-redux';

import firebase from 'firebase/app';
import { useEffect } from 'react/cjs/react.development';

// Manages the login for firebase
export const FirebaseManager = function() {

  const dispatch = useDispatch();
  const clientToken = useSelector(selectClientCustomToken);

  useEffect(() => {
    dispatch(getClientCustomToken());
  }, [dispatch]);

  useEffect(() => {
    let refreshTokenInterval;

    async function signIn(){
      if(clientToken){
        try{
          await firebase.auth().signInWithCustomToken(clientToken)
          dispatch(setFirebaseAuthenticated());

          // Token expires after 1 hour.
          refreshTokenInterval = setInterval(() => {
            console.log("refeshing")
            dispatch(getClientCustomToken());
          }, process.env.REACT_APP_FIREBASE_TOKEN_REFRESH_MS);
        }
        catch(ex){
        }
      }
    }

    signIn();

    return (() => {
      clearInterval(refreshTokenInterval);
    });
  }, [dispatch, clientToken]);


  return null;
}


