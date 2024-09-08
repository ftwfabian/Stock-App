
import App from './App.svelte';
import './global.css';

//Obviously I do not have a UserAPI, that will need to be set up
//let name =  fetch(`/api/UserApi/GetUserId`);

const app = new App({
  target: document.body,
  props: {
    name: 'Harrison',
    url: window.location.pathname
  }
});

export default app;

//...Now, everything should be defined in the App.svelte file.