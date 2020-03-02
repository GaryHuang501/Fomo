import React, { Component } from 'react';
import './PortfolioSymbol.css';

export class PortfolioSymbol extends Component {

    constructor(props) {
        super(props);
        this.actions = {

        };

    }

    componentDidMount() {
    }

    showAlerts(){
        console.log(this.actions);
    }

    showGraph(){

    }

    showSymbolInfo(){

    }

    removeSymbol(){

    }

    render() {

        const portoflioSymbol = this.props.portfolioSymbol || {};

        return (
            
            <li key={portoflioSymbol.symbolName} className='portfolio-symbol'>
                <span className='portfolio-symbol-name portfolio-symbol-column'>{portoflioSymbol.symbolName}</span>
                <span className='portfolio-symbol-change portfolio-symbol-column'>{portoflioSymbol.symbolName}</span>
                <span className='portfolio-symbol-value portfolio-symbol-column'>{portoflioSymbol.symbolName}</span>
                <span className='portfolio-symbol-actions portfolio-symbol-column'>
                    <ul >
                        <li>Alerts</li>
                        <li>Graph</li>
                        <li>Info</li>
                        <li>Remove</li>
                    </ul>
                </span>
            </li>
        );
    }


}
