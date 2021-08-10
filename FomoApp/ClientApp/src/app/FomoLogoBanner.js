import './FomoLogoBanner.css';
import '../assets/fontawesome-free-5.14.0-web/css/fontawesome.min.css';
import '../assets/fontawesome-free-5.14.0-web/css/solid.min.css';
import '../assets/fontawesome-free-5.14.0-web/css/regular.min.css';

import {React} from 'react';

// Navigation header bar for changing pages on main app page
export default function FomoLogoBanner(props) {
  
  return (
    <header id='fomo-logo-banner'>
          <h2 id='fomo-logo-banner-title'>FOMO <span className="check icon"></span></h2>
          {props.children}
    </header>
  );
}
