import React, { useState, useEffect } from 'react';
import {
  BrowserRouter as Router,
  Switch,
  Route,
  Redirect,
} from 'react-router-dom'
import { axiosSetup } from './app/AxiosSetup';
import { NavHeader } from './app/NavHeader';
import { PortfolioPage } from './features/portfolio/PortfolioPage';
import { LeaderBoardPage } from './features/leaderboard/LeaderBoardPage';
import { MembersPage } from './features/members/MembersPage';
import { LoginModal } from './features/login/LoginModal';
import { useSelector, useDispatch } from 'react-redux'
import './App.css';
import { checkLogin, selectAuthenticatedState } from './features/login/LoginSlice';

function App() {

  const dispatch = useDispatch();
  const [isSetupFinished, setSetupFinished] = useState(false);
  const isAuthenticated = useSelector(selectAuthenticatedState);

  useEffect(() => {
    axiosSetup(dispatch);
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
