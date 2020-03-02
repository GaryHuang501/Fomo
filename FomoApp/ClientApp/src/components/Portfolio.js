import React, { Component } from 'react';
import { PortfolioSymbolSearchBox } from './PortfolioSymbolSearchBox';
import { PortfolioSymbolList } from './PortfolioSymbolList';
import './Portfolio.css';

export class Portfolio extends Component {

    constructor(props) {
        super(props);
    }

    componentDidMount() {
        this.props.onLoadSymbolForPortfolio("123");
    }

    render() {
        const selectedPortfolioId = this.props.selectedPortfolioId;

        return (
            <div id='portfolio-wrapper'>
                <PortfolioSymbolSearchBox
                    exchanges={this.props.exchanges}
                    addSymbol={(exchange, symbol) =>
                        this.props.onAddSymbolToPortfolio(selectedPortfolioId, exchange, symbol)}
                />
                <PortfolioSymbolList
                    selectedPortfolioSymbols={this.props.portfoliosToSymbols[selectedPortfolioId]}
            />
            </div>
        );
    }


}
