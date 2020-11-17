import React, { useState, useEffect, useRef } from 'react';
import '../../assets/fontawesome-free-5.14.0-web/css/fontawesome.min.css';
import '../../assets/fontawesome-free-5.14.0-web/css/solid.min.css';
import { DebounceInput } from 'react-debounce-input';
import { useDispatch, useSelector} from 'react-redux'
import './StockSearchBar.css';
import { StockSearchMatch } from './StockSearchMatch'
import { searchStocks, selectStockSearchResults } from './StockSearchSlice'

export const StockSearchBar = function () {

  const thisStockSearchBarRef = useRef();
  const dispatch = useDispatch();
  const stockSearchResults = useSelector(selectStockSearchResults);
  const [searchKeywords, setSearchKeywords] = useState('');

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

  function onClickStockMatch(symbol){
    console.log(symbol);
    setSearchKeywords('');
  }

  function isSearchKeywordsSet(){
    return searchKeywords && searchKeywords.length > 0;
  }

  // Create the JSX for the search results drop down to render.
  function createSearchResultsBox(){
    let resultBox = null;
    const potentialMatches = [];
  
    const searchResultExists = stockSearchResults && isSearchKeywordsSet() && searchKeywords in stockSearchResults;

    if (!searchResultExists) {
      return null;
    }

    for (const result of stockSearchResults[searchKeywords]) {
      potentialMatches.push(<StockSearchMatch key={result.symbol} symbol={result.symbol} fullName={result.fullName} onClick={onClickStockMatch}/>)
    }
      
    const hasMatches = potentialMatches.length > 0;
  
    if(hasMatches){
      resultBox = <div id='stock-search-results'>{potentialMatches}</div>
    }
    else{
      resultBox = <div id='stock-search-results'><StockSearchMatch key={null} symbol={null} fullName={"No results found."} /></div>
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
