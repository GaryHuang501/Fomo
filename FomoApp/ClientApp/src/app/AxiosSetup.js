import axios from 'axios';
import { setUnauthenticated } from '../features/login/LoginSlice';

export const axiosSetup = (dispatch) => {

  axios.defaults.withCredentials = true;
  
  axios.interceptors.response.use(function (response) {
    return response;
  }, function (error) {

    if(error.response.status === '401' || error.response.status === '404'){
      console.log("unauthenticated");
      dispatch(setUnauthenticated());
    }    
    
    return Promise.reject(error);
  }); 
}
