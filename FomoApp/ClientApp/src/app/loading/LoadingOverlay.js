import './LoadingOverlay.css';

import { React } from 'react';

// Overlay that shows a loading indicator in the center of the parent container.
export const LoadingOverlay = function (props) {

  return (
    <div class='loading-overlay'>
      <div class="lds-ring"><div></div><div></div><div></div><div></div></div>    
  </div> 
  );
}
