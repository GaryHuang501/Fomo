import './FaqPage.css';

import {
  Link,
  Route,
  BrowserRouter as Router,
  Switch,
  useRouteMatch,
} from "react-router-dom";

import AboutFaq from './AboutFaq';
import LeaderBoardFaq from './LeaderBoardsFaq';
import PortfolioFaq from './PortfolioFaq';
import React from 'react';
import SocialFaq from './SocialFaq';

export default function FaqPage() {

  const match = useRouteMatch();

  return (
    <main id="faq-page">
      <div id="faq-container">
        <Router>
            <div id="faq-questions">
              <ul>
                <li><Link to={`${match.url}/About`}>What is Fomo App about?</Link></li>
                <li>
                  <p>Features</p>
                  <ul>
                    <li><Link to={`${match.url}/Portfolio`}>Portfolio Management</Link></li>
                    <li><Link to={`${match.url}/LeaderBoards`}>Leader Boards</Link></li>
                    <li><Link to={`${match.url}/Social`}>Social Features</Link></li>
                  </ul>
                </li>
              </ul>
          </div>
          <div id="faq-answers">
            <Switch>
              <Route path={`${match.path}/About`}>
                <AboutFaq />
              </Route>
              <Route path={`${match.path}/Portfolio`}>
                <PortfolioFaq />
              </Route>
              <Route path={`${match.path}/LeaderBoards`}>
                <LeaderBoardFaq />
              </Route>
              <Route path={`${match.path}/Social`}>
                <SocialFaq />
              </Route>
              <Route path={`${match.path}/`}>
                <AboutFaq />
              </Route>
            </Switch>
          </div>
        </Router>
      </div>
    </main>
  );
}
