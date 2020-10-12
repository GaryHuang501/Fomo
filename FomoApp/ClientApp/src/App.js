import React from 'react';
import {
  BrowserRouter as Router,
  Switch,
  Route,
  Redirect,
} from 'react-router-dom'
import { library } from '@fortawesome/fontawesome-svg-core'
import { fab } from '@fortawesome/free-brands-svg-icons'
import { faCheckSquare, faCoffee } from '@fortawesome/free-solid-svg-icons'

import { NavHeader } from './app/NavHeader';
import { PortfolioPage } from './features/portfolio/PortfolioPage';
import { LeaderBoardPage } from './features/leaderboard/LeaderBoardPage';
import { MembersPage } from './features/members/MembersPage';

import './App.css';

function App() {
  return (
     <Router>
        <NavHeader></NavHeader>
        <Switch>
          <Route exact path="/" component={PortfolioPage}/>
          <Route exact path="/Leaderboard" component={LeaderBoardPage} />
          <Route exact path="/Members" component={MembersPage} />
          <Route exact path="/About" component={MembersPage} />
          <Redirect to="/" />
        </Switch>
    </Router>
  );
}

export default App;
