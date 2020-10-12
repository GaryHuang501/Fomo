import React from 'react';
import './StockSearchBar.css';
import '../../assets/fontawesome-free-5.14.0-web/css/fontawesome.min.css';
import '../../assets/fontawesome-free-5.14.0-web/css/solid.min.css';

export const StockSearchBar = function () {
  return (
    <form id='stock-search-bar'>
      <i className="fas fa-search-plus search-icon "></i>
      <input type="search" placeholder='Search for US stocks...'></input>
    </form>
  );
}