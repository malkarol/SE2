import React, { useState } from "react";
import { withRouter } from "react-router";

function MainPage(props)
{
  console.log(props);
  return(
    <div>
      <h3>Hello {props.location.state.user}, welcome to E-voting.</h3>
    </div>
  )
}
export default (withRouter(MainPage));