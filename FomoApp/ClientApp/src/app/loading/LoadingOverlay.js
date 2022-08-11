import './LoadingOverlay.css';

import { React } from 'react';

// Overlay that shows a loading indicator in the center of the parent container.
export const LoadingOverlay = function (props) {

  return (
    <div className='loading-overlay'>
      <div className="lds-ring"><div></div><div></div><div></div><div></div></div>    
  </div> 
  );
}
