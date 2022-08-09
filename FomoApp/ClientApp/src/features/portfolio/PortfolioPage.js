import './PortfolioPage.css';

import React, { useEffect } from 'react';
import { fetchPortfolio, fetchPortfolioIds, selectPortfolioIds, setSelectedPortfolioId } from './PortfolioSlice';
import { getAccountForId, selectMyUser, selectUser } from '../login/LoginSlice'
import { useDispatch, useSelector } from 'react-redux'

import ChatBox from '../chatbox/ChatBox';
import { LoadingOverlay } from '../../app/loading/LoadingOverlay';
import { Portfolio } from './Portfolio';
import PortfolioListener from './PortfolioListener';
import ShareProfileLink from "./ShareProfileLink";
import { StockSearchBar } from '../stockSearch/StockSearchBar';
import { TickerTape } from './TickerTape';
import { useParams } from "react-router-dom";

export default function PortfolioPage() {

  const dispatch = useDispatch();
  const params = useParams();
  
  const urlUserId = params.urlUserId;
  const myUser = useSelector(selectMyUser);
  const selectedUser = useSelector(selectUser);
  const selectedPortfolioUserId = urlUserId ?? myUser.id;
  const isMyUserPage = selectedPortfolioUserId === myUser.id;
  const portfolioIds = useSelector(selectPortfolioIds);
  
  useEffect(() => {
    dispatch(getAccountForId(selectedPortfolioUserId));
  }, [dispatch, selectedPortfolioUserId]);

  useEffect(() => {
    dispatch(fetchPortfolioIds(selectedPortfolioUserId));
  }, [dispatch, selectedPortfolioUserId]);

  useEffect(() => {
    // Currently only support one portfolio per user
    if(portfolioIds.length > 0){
      dispatch(setSelectedPortfolioId(portfolioIds[0]));
      dispatch(fetchPortfolio(portfolioIds[0]));
    }
  }, [portfolioIds, dispatch]);
  
  return (
    <main id="portfolio-page">
        <section id='portfolio-stock-search-container'>
          { isMyUserPage ? <StockSearchBar></StockSearchBar> : null }
        </section>
        <section id='portfolio-ticker-tape-container'>
          <TickerTape></TickerTape>
        </section>
        <section id='portfolio-container'>
          <PortfolioListener></PortfolioListener>
          <Portfolio isMyUserPage={isMyUserPage}></Portfolio>
        </section>
            {
              selectedUser != null ? 
              <section id='portfolio-chatbox-container'>
                  <h3 id='portfolio-chatbox-header'><ShareProfileLink userId={selectedUser.id}/><span id='portfolio-chatbox-header-title'>{selectedUser.name}</span></h3>
                  <ChatBox myUser={myUser} selectedUser={selectedUser}></ChatBox>
              </section>
                  : null 
            }
        <footer id='portfolio-footer'></footer>
    </main>
  );
}