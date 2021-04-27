import {
  BrowserRouter as Router,
  Switch,
  Route
} from "react-router-dom";
import { applyMiddleware, createStore } from 'redux';
import { composeWithDevTools } from 'redux-devtools-extension';
import reduxThunk from 'redux-thunk';
import { Provider } from 'react-redux';
import AppReducers from './AppReducers/AppReducers'
import LoginPage from "./Components/LoginPage";
import MainPage from "./Components/MainPage";
import './App.css';

const store = createStore(AppReducers, {}, composeWithDevTools((applyMiddleware(reduxThunk))));

function App() {
  return (
    <Provider store = {store}>
        <Router>
            <Switch>
                <Route exact path="/login">
                    <LoginPage />
                </Route>
                <Route exact path="/">
                    <MainPage />
                </Route>
                <Route exact path="/ManagementPanel">
                    <MainPage isCoordinator/>
                </Route>
            </Switch>
        </Router>
    </Provider>
  );
}

export default App;
