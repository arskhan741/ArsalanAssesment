import jwt from "jsonwebtoken";

// Send login request and Get Proper API Response
document.getElementById("loginBtn").addEventListener("click", async () => {
  const email = document.getElementById("UsernameValue").value;
  const password = document.getElementById("passwordValue").value;

  // If any field is empty throw
  if (!email || !password) {
    throw new Error(`Please enter valid email or password`);
  }

  // Create Login details JSON
  const loginDetails = {
    email: email,
    password: password,
  };

  // Try to get login api response
  try {
    const loginResponse = await sendLoginRequest(loginDetails);

    let loggedInUser = new LoggedInUser(
      loginDetails,
      loginResponse.result,
      true
    );
    console.log(`loggedInUser = ${loggedInUser}`);
  } catch (error) {
    // handle response for invalid logged in conditions
  }
});

// Configure headers and send API request for login
const sendLoginRequest = async (loginDetails) => {
  const myHeaders = new Headers();
  myHeaders.append("Content-Type", "application/json");

  return FetchApiResponse(
    myHeaders,
    `https://localhost:7207/api/users/login`,
    loginDetails
  );
};

async function FetchApiResponse(headers, url, body) {
  try {
    const response = await fetch(url, {
      method: `POST`,
      headers: headers,
      body: JSON.stringify(body),
    });

    if (!response.ok) {
      throw new Error(`Response status: ${response.status}`);
    }

    const json = await response.json();
    console.log(`response for ${url}`, json);

    return json;
  } catch (error) {
    console.error(`API request ${url} error:`, error);
  }
}

class LoggedInUser {
  constructor(loginDetails, jwtToken, loggedIn) {
    this.email = loginDetails.email;
    this.password = loginDetails.password;
    this.jwtToken = jwtToken;
    this.loggedIn = loggedIn;

    // Decode the JWT to extract the User ID
    this.userId = this.extractUserId(jwtToken);
  }

  extractUserId(token) {
    try {
      // Check if the jwt library is loaded
      if (typeof window.jwt === "undefined") {
        throw new Error("jsonwebtoken library not loaded");
      }

      // Decode the token without verifying the signature
      const decoded = window.jwt.decode(token); // Use window.jwt
      return decoded ? decoded.UserGuid : null; // Return UserGuid or null if not found
    } catch (error) {
      console.error("Error decoding JWT:", error);
      return null; // Return null in case of an error
    }
  }
}
