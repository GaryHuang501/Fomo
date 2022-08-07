import './TickerTape.css';

import React from 'react';
import { useEffect } from 'react/cjs/react.development';

export const TickerTape = () => {

  useEffect(() => {
    const settings = {
      "symbols": [
        {
          "proName": "FOREXCOM:SPXUSD",
          "title": "S&P 500"
        },
        {
          "proName": "FOREXCOM:NSXUSD",
          "title": "Nasdaq 100"
        },
        {
          "proName": "BITSTAMP:BTCUSD",
          "title": "BTC/USD"
        },
        {
          "description": "TSX",
          "proName": "TSX:TSX"
        },
        {
          "description": "CAD/USD",
          "proName": "FX_IDC:CADUSD"
        }
      ],
      "showSymbolLogo": true,
      "colorTheme": "light",
      "isTransparent": false,
      "displayMode": "adaptive",
      "locale": "en"
    }

    const script = document.createElement('script');
    script.src = 'https://s3.tradingview.com/external-embedding/embed-widget-ticker-tape.js'
    script.async = true;
    script.innerHTML = JSON.stringify(settings);
    document.getElementById('ticker-tape').appendChild(script);

    return () => {

      const tickerTape = document.getElementById('ticker-tape');

      if(tickerTape)
      {
        tickerTape.removeChild(script);
      }
    }
  }, []);

  return (
    <div id='ticker-tape'>
      <div className="tradingview-widget-container">
        <div className="tradingview-widget-container__widget"></div>
        <div className="tradingview-widget-copyright"></div>
      </div>
    </div>
  );
}