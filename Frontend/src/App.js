import {
  BrowserRouter as Router,
  Switch,
  Route
} from "react-router-dom";
import './App.css';
import LoginPage from "./Components/LoginPage";
import MainPage from "./Components/MainPage";


function App() {
  return (
    <Router>
      <Switch>
        <Route path="/login" exact>
          <LoginPage />
        </Route>
        <Route path="/" exact>
          <MainPage />
        </Route>
        <Route path="/" exact>
          <MainPage />
        </Route>
      </Switch>
    </Router>
  );
}

export default App;
