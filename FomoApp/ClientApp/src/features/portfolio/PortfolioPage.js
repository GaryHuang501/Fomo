import React from 'react';
import { StockSearchBar } from '../stockSearch/StockSearchBar';
import { Portfolio } from './Portfolio';
import { ChatBox } from '../chatbox/ChatBox';
import './PortfolioPage.css';

export const PortfolioPage = function() {

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