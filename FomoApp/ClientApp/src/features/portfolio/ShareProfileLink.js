import './ShareProfileLink.css';
import '../../assets/fontawesome-free-5.14.0-web/css/fontawesome.min.css';
import '../../assets/fontawesome-free-5.14.0-web/css/solid.min.css';
import '../../assets/fontawesome-free-5.14.0-web/css/regular.min.css';

import React, {useState} from 'react';

export default function ShareProfileLink (props){

  const [showToolTip, setShowToolTip] = useState(false);
  
  async function copyLink() {

    setShowToolTip(true);

    const profileLink = `${window.location.origin}/portfolio/${props.userId}`;
    await navigator.clipboard.writeText(profileLink);
    setTimeout(function(){ setShowToolTip(false)}, 1500);
  }

  return (
    <span id="share-profile-link" >
      { showToolTip ? <div id='share-profile-link-tooltip' role='alert'>Profile Link Copied</div> : null }
      <i className="fas fa-share-square" onClick={copyLink} role="button" />
    </span>
  );
}