import React, { Component } from 'react';
import './PortfolioSymbolSearchBox.css';

export class PortfolioSymbolSearchBox extends Component {

    constructor(props) {
        super(props);

        this.state = {
            symbolSearchInputValue: "",
            selectedExchange: ""
        };
    }

    onSymbolSearchValueChanged(value) {
        this.setState({
            symbolSearchInputValue: value
        });
    }

    clearSymbolSearchValue() {
        this.setState({
            symbolSearchInputValue: ""
        });
    }

    onSelectedExchangeChanged(value) {
        this.setState({
            selectedExchange: value
        });
    }

    addSymbolToPortfolio(e) {
        e.preventDefault();

        if (this.state.selectedExchange.length === 0 || this.state.symbolSearchInputValue.length === 0) {
            return;
        }

        this.clearSymbolSearchValue();
        this.props.addSymbol(this.state.selectedExchange, this.state.symbolSearchInputValue);
    }

    render() {

        let exchangeList = this.props.exchanges.map(e =>
            <option key={e.exchangeId} value={e.exchangeId}> {e.exchangeName} </option>
        );
        
        return (
            <div id='portfolio-symbol-search-box'>
                <form id='portfolio-symbol-search-box-form' onSubmit={e => this.addSymbolToPortfolio(e)}>
                    <select id='portfolio-symbol-search-exchange-selection' onChange={e => this.onSelectedExchangeChanged(e.target.value)} value={this.props.exchanges[0].exchangeId}>
                            {exchangeList}
                        </select>
                        <input type='text' id='portfolio-symbol-search-box-text-input' placeholder='Search Symbol' onChange={e => this.onSymbolSearchValueChanged(e.target.value)} value={this.state.symbolSearchInputValue}/>
                        <input type='submit' value='+' id='portfolio-symbol-search-box-submit-button' />
                    </form>
            </div>
        );
    }


}
