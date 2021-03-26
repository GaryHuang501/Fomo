import './App.css';

import React, { useEffect } from 'react';
import {
  Redirect,
  Route,
  BrowserRouter as Router,
  Switch,
} from 'react-router-dom'
import { getAccount, selectAuthenticatedState, selectFirebaseAuthenticatedState, signOut } from './features/login/LoginSlice';
import { useDispatch, useSelector } from 'react-redux'

import { FirebaseManager } from './app/FirebaseManager';
import { LeaderBoardPage } from './features/leaderboard/LeaderBoardPage';
import { LoginModal } from './features/login/LoginModal';
import { MembersPage } from './features/members/MembersPage';
import { NavHeader } from './app/NavHeader';
import { PortfolioPage } from './features/portfolio/PortfolioPage';
import { axiosSetup } from './app/AxiosSetup';
import { firebaseSetup } from './app/FirebaseSetup';

function App() {

  const dispatch = useDispatch();
  const isAuthenticated = useSelector(selectAuthenticatedState);
  const isFirebaseAuthenticated = useSelector(selectFirebaseAuthenticatedState);

  function signOut(){
    window.location.href = `${process.env.REACT_APP_API_URL}/accounts/logout?returnurl=${window.location.href}`;
  }

  useEffect(() => {
    
    function setupHttpClient(){
      axiosSetup(dispatch);
    }
  
    setupHttpClient();
    firebaseSetup();
    dispatch(getAccount());
  }, [dispatch]);

  return (
     <Router>
        <NavHeader signOut={signOut}></NavHeader>
        { !isAuthenticated ? <LoginModal></LoginModal> : null }
        { isAuthenticated ? <FirebaseManager></FirebaseManager> : null } 
        { isFirebaseAuthenticated ?
          <Switch>
            <Route exact path="/" component={PortfolioPage}/>
            <Route exact path="/Leaderboard" component={LeaderBoardPage} />
            <Route exact path="/Members" component={MembersPage} />
            <Route exact path="/About" component={MembersPage} />
            <Redirect to="/" />
          </Switch> : null}
    </Router>
  );
}

export default App;
