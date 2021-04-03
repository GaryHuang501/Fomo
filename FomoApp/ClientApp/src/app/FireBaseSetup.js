import 'firebase/auth';
import 'firebase/database';
import 'firebase/analytics';

import firebase from 'firebase/app';

export const firebaseSetup = function(){
    var firebaseConfig = {
        apiKey: process.env.REACT_APP_FIREBASE_API_KEY,
        authDomain: process.env.REACT_APP_FIREBASE_AUTH_DOMAIN,
        databaseURL: process.env.REACT_APP_FIREBASE_DATABASE_URL,
        projectId: process.env.REACT_APP_FIREBASE_PROJECTID,
        storageBucket: process.env.REACT_APP_FIREBASE_STORAGE_BUCKET,
        messagingSenderId: process.env.REACT_APP_FIREBASE_MESSAGESENDERID,
        appId: process.env.REACT_APP_FIREBASE_APPID,
        measurementId: process.env.REACT_APP_FIREBASE_MEASUREMENTID
      };
    
      if(firebase.apps.length === 0){
        firebase.initializeApp(firebaseConfig);
      }
      
      firebase.analytics();
}
