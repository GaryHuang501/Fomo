import './App.css';

import React, { useEffect, useState } from 'react';
import {
  Redirect,
  Route,
  BrowserRouter as Router,
  Switch,
} from 'react-router-dom'
import { checkLogin, selectAuthenticatedState } from './features/login/LoginSlice';
import { useDispatch, useSelector } from 'react-redux'

import { LeaderBoardPage } from './features/leaderboard/LeaderBoardPage';
import { LoginModal } from './features/login/LoginModal';
import { MembersPage } from './features/members/MembersPage';
import { NavHeader } from './app/NavHeader';
import { PortfolioPage } from './features/portfolio/PortfolioPage';
import { axiosSetup } from './app/AxiosSetup';
import { firebaseSetup } from './app/FireBaseSetup';

function App() {

  const dispatch = useDispatch();
  const [isSetupFinished, setSetupFinished] = useState(false);
  const isAuthenticated = useSelector(selectAuthenticatedState);

  useEffect(() => {
    
    function setupHttpClient(){
      axiosSetup(dispatch);
    }
  
    function setupNotifications(){
      firebaseSetup();
    }

    setupNotifications();
    setupHttpClient();
    dispatch(checkLogin());
    setSetupFinished(true);
  }, [dispatch]);

  return (
     <Router>
        <NavHeader></NavHeader>
        { isSetupFinished && !isAuthenticated ? <LoginModal></LoginModal> : null }
        { isSetupFinished && isAuthenticated ?
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
