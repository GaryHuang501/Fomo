import React, { useEffect } from 'react';
import { StockSearchBar } from '../stockSearch/StockSearchBar';
import { Portfolio } from './Portfolio';
import { ChatBox } from '../chatbox/ChatBox';
import './PortfolioPage.css';
import { useDispatch, useSelector } from 'react-redux'
import { fetchPortfolioIds, fetchPortfolio, setSelectedPortfolioId, selectPortfolioIds, selectSelectedPortfolioId } from './PortfolioSlice';

export const PortfolioPage = function() {

  const dispatch = useDispatch();
  const portfolioIds = useSelector(selectPortfolioIds);
  const selectedPortfolioId = useSelector(selectSelectedPortfolioId);

  useEffect(() => {
    dispatch(fetchPortfolioIds());
  }, [dispatch]);

  useEffect(() => {
    // Currently only support one portfolio per user
    if(portfolioIds.length > 0){
      dispatch(setSelectedPortfolioId(portfolioIds[0]));
    }
  }, [portfolioIds, dispatch]);

  useEffect(() => {
    if(selectedPortfolioId > 0){
      dispatch(fetchPortfolio(selectedPortfolioId))
    }
  }, [selectedPortfolioId, dispatch]);

  return (
    <main id="portfolio-page">
        <section id='portfolio-stock-search-container'>
            <StockSearchBar></StockSearchBar>
        </section>
        <section id='portfolio-container'>
            <Portfolio></Portfolio>
        </section>
        <section id='portfolio-chatbox-container'>
            <h3 id='portfolio-chatbox-header'>Gary's Portfolio</h3>
            <ChatBox></ChatBox>
        </section>
    </main>
  );
}