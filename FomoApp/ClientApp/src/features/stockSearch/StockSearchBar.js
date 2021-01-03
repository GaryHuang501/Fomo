import '../../assets/fontawesome-free-5.14.0-web/css/fontawesome.min.css';
import '../../assets/fontawesome-free-5.14.0-web/css/solid.min.css';
import './StockSearchBar.css';

import React, { useEffect, useRef, useState } from 'react';
import { addPortfolioStock, selectSelectedPortfolioId } from '../portfolio/PortfolioSlice'
import { searchStocks, selectStockSearchResults, selectStockSearchStatus } from './StockSearchSlice'
import { useDispatch, useSelector } from 'react-redux'

import { DebounceInput } from 'react-debounce-input';
import { StockSearchMatch } from './StockSearchMatch'

export const StockSearchBar = function () {

  const thisStockSearchBarRef = useRef();
  const dispatch = useDispatch();
  const [searchKeywords, setSearchKeywords] = useState('');

  const stockSearchResults = useSelector(state => selectStockSearchResults(state, searchKeywords));
  const selectedPortfolioId = useSelector(selectSelectedPortfolioId);
  const stockSearchStatus = useSelector(selectStockSearchStatus);

  const closeResultsWhenOutsideClick = e => {
    if (thisStockSearchBarRef.current.contains(e.target)) {
      return;
    }

    setSearchKeywords('');
  };
  
  useEffect(() => {
    document.addEventListener("mousedown", closeResultsWhenOutsideClick);

    return () => {
      document.removeEventListener("mousedown", closeResultsWhenOutsideClick);
    };
  }, []);

  function onKeywordChange(event) {
    dispatch(searchStocks(event.target.value));
    setSearchKeywords(event.target.value);
  }

  function onSubmit(event) {
    event.preventDefault();
  }

  function onClickStockMatch(symbolId){
    setSearchKeywords('');
    dispatch(addPortfolioStock({symbolId: symbolId, portfolioId: selectedPortfolioId}));
  }

  // Create the JSX for the search results drop down to render.
  function createSearchResultsBox(){
    let resultBox = null;
    const potentialMatches = [];

    for (const result of stockSearchResults) {
      const match = <StockSearchMatch 
                      key={result.symbolId} 
                      match={result}
                      onClick={onClickStockMatch}/>;

      potentialMatches.push(match);
    }
      
    const hasMatches = potentialMatches.length > 0;
  
    if(hasMatches){
      resultBox = <div id='stock-search-results'>{potentialMatches}</div>
    }
    else if(stockSearchStatus  === 'loading'){
      resultBox = <div id='stock-search-results'><StockSearchMatch key={null} match={{ ticker: null, fullName: "Searching..."}} /></div>
    }
    else if(searchKeywords.trim().length > 0 && stockSearchStatus  === 'succeeded'){
      resultBox = <div id='stock-search-results'><StockSearchMatch key={null} match={{ ticker: null, fullName: "No results found."}} /></div>
    }
    
    return resultBox;
  }

  const searchResultsBox = createSearchResultsBox();

  return (
    <div id='stock-search-bar' ref={thisStockSearchBarRef}>
      <form id='stock-search-bar-form' onSubmit={onSubmit}>
        <i className="fas fa-search-plus search-icon "></i>
        <DebounceInput minLength={1} debounceTimeout={500} type="search" placeholder='Search for US stocks...' onChange={onKeywordChange} value={searchKeywords}></DebounceInput>
      </form>
      {searchResultsBox}
    </div>
  );
}
