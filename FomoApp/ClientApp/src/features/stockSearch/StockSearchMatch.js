import React from 'react';
import './StockSearchMatch.css';

export const StockSearchMatch = function (props) {

  function clickCallback(){
    if(props.onClick){
      props.onClick(props.symbol)
    }
  }

  return (
    <div className='stock-search-match' onClick={clickCallback}>
        <span className='stock-search-match-symbol'>{props.symbol}</span>
        <span className='stock-search-match-fullname'>{props.fullName}</span>
    </div>
  );
}
