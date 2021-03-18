import 'firebase/auth';
import 'firebase/database';

import {getClientCustomToken, selectClientCustomToken, setFireBaseAuthenticated} from  '../features/login/LoginSlice';
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
    async function signIn(){
      if(clientToken){
        try{
          await firebase.auth().signInWithCustomToken(clientToken)
          dispatch(setFireBaseAuthenticated());
        }
        catch(ex){
        }
      }
    }
    
    signIn();
  }, [dispatch, clientToken]);


  return null;
}


