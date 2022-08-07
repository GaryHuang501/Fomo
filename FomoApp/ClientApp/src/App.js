import './App.css';

import React, { useEffect } from 'react';
import {
  Route,
  Routes,
} from 'react-router-dom'
import { getAccount, selectAuthenticatedState, selectFirebaseAuthenticatedState } from './features/login/LoginSlice';
import { useDispatch, useSelector } from 'react-redux'

import FaqPage  from './features/faq/FaqPage';
import { FirebaseManager } from './app/FirebaseManager';
import  LeaderBoardPage  from './features/leaderboard/LeaderBoardPage';
import { LoginModal } from './features/login/LoginModal';
import MembersPage  from './features/members/MembersPage';
import { NavHeader } from './app/NavHeader';
import PortfolioPage  from './features/portfolio/PortfolioPage';
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
    <React.Fragment>
         <div id='modal-root'></div>
        <NavHeader signOut={signOut}></NavHeader>
        { !isAuthenticated ? <LoginModal></LoginModal> : null }
        { isAuthenticated ? <FirebaseManager></FirebaseManager> : null } 
        { isFirebaseAuthenticated ?
          <Routes>
            <Route path="/" element={<PortfolioPage/>}/>
            <Route path="portfolio" element={<PortfolioPage/>}/>
            <Route path="portfolio/:urlUserId" element={<PortfolioPage/>}/>
            <Route path="Leaderboard" element={<LeaderBoardPage/>} />
            <Route path="Friends" element={<MembersPage/>} />
            <Route path="Faq/*" element={<FaqPage/>} />
            <Route path="*" element={<App/>}/>
          </Routes> : null}
    </React.Fragment>
  );
}

export default App;
