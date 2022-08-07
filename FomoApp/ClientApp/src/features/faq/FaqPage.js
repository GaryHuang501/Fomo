import './FaqPage.css';

import {
  Link,
  Route,
  Routes,
} from "react-router-dom";

import AboutFaq from './AboutFaq';
import LeaderBoardFaq from './LeaderBoardsFaq';
import PortfolioFaq from './PortfolioFaq';
import React from 'react';
import SocialFaq from './SocialFaq';

export default function FaqPage() {

  return (
    <main id="faq-page">
      <div id="faq-container">
            <div id="faq-questions">
              <ul>
                <li><Link to={`About`}>What is Fomo App about?</Link></li>
                <li>
                  <p>Features</p>
                  <ul>
                    <li><Link to={`Portfolio`}>Portfolio Management</Link></li>
                    <li><Link to={`LeaderBoards`}>Leader Boards</Link></li>
                    <li><Link to={`Social`}>Social Features</Link></li>
                  </ul>
                </li>
              </ul>
          </div>
          <div id="faq-answers">
            <Routes>
              <Route path={`About`} element={<AboutFaq/>}>
                
              </Route>
              <Route path={`Portfolio`} element={<PortfolioFaq/>}>
              </Route>
              <Route path={`/LeaderBoards`} element={<LeaderBoardFaq/>}>
              </Route>
              <Route path={`/Social`} element={<SocialFaq/>}>
              </Route>
              <Route path={`/*`} element={<AboutFaq/>}>
              </Route>
            </Routes>
          </div>
      </div>
    </main>
  );
}
