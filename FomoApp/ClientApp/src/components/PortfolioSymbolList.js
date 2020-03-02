import React, { Component } from 'react';
import './PortfolioSymbolList.css';
import { PortfolioSymbol } from './PortfolioSymbol';

export class PortfolioSymbolList extends Component {

    constructor(props) {
        super(props);
    }

    componentDidMount() {
    }

    render() {

        const selectedPortfolioSymbols = this.props.selectedPortfolioSymbols || [];
        const portfolioSymbolListView = selectedPortfolioSymbols.map(ps =>
            <PortfolioSymbol key={ps.symbolName} portfolioSymbol={ps}/>
        );

        return (
            <ul id='portfolio-symbol-list'>
                {portfolioSymbolListView}
            </ul>
        );
    }


}
