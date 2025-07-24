const api_url = "http://localhost:10010";

const handleError = async (response, fallbackMessage) => {
  let errorMsg = fallbackMessage;

  try {
    const contentType = response.headers.get("content-type");

    if (contentType?.includes("application/json")) {
      const json = await response.json();
      if (json?.exception?.message) errorMsg = json.exception.message;
    } else {
      const text = await response.text();
      if (text) errorMsg = text;
    }
  } catch (fallbackError) {
    console.warn("Error extracting backend error message:", fallbackError);
  }

  throw new Error(errorMsg);
};

export const startPlan = async () => {
  const url = `${api_url}/Plan`;

  try {
    const response = await fetch(url, {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({}),
    });

    if (!response.ok) await handleError(response, "Failed to create plan");

    return await response.json();
  } catch (err) {
    console.error("startPlan error:", err);
    throw err;
  }
};

export const addProcedureToPlan = async (planId, procedureId) => {
  const url = `${api_url}/Plan/AddProcedureToPlan`;
  const command = { planId, procedureId };

  try {
    const response = await fetch(url, {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify(command),
    });

    if (!response.ok)
      await handleError(response, "Failed to add procedure to plan");

    return true;
  } catch (err) {
    console.error("addProcedureToPlan error:", err);
    throw err;
  }
};

export const getProcedures = async () => {
  const url = `${api_url}/Procedures`;

  try {
    const response = await fetch(url);
    if (!response.ok) await handleError(response, "Failed to get procedures");
    return await response.json();
  } catch (err) {
    console.error("getProcedures error:", err);
    throw err;
  }
};

export const getPlanProcedures = async (planId) => {
  const url = `${api_url}/PlanProcedure?$filter=planId eq ${planId}&$expand=procedure`;

  try {
    const response = await fetch(url);
    if (!response.ok)
      await handleError(response, "Failed to get plan procedures");
    return await response.json();
  } catch (err) {
    console.error("getPlanProcedures error:", err);
    throw err;
  }
};

export const getUsers = async () => {
  const url = `${api_url}/Users`;

  try {
    const response = await fetch(url);
    if (!response.ok) await handleError(response, "Failed to get users");
    return await response.json();
  } catch (err) {
    console.error("getUsers error:", err);
    throw err;
  }
};

export const getUsersForProcedure = async (planId, procedureId) => {
  const url = `${api_url}/api/plan-procedures/${planId}/${procedureId}`;

  try {
    const response = await fetch(url);
    if (!response.ok)
      await handleError(response, "Failed to get assigned users");
    return await response.json();
  } catch (err) {
    console.error("getUsersForProcedure error:", err);
    throw err;
  }
};

export const assignUserToProcedure = async (planId, procedureId, userId) => {
  const url = `${api_url}/api/plan-procedures`;

  try {
    const response = await fetch(url, {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ planId, procedureId, userId }),
    });

    if (!response.ok) await handleError(response, "Failed to assign user");
  } catch (err) {
    console.error("assignUserToProcedure error:", err);
    throw err;
  }
};

export const removeUserFromProcedure = async (planId, procedureId, userId) => {
  const url = `${api_url}/api/plan-procedures/${planId}/${procedureId}/${userId}`;

  try {
    const response = await fetch(url, {
      method: "DELETE",
    });

    if (!response.ok) await handleError(response, "Failed to remove user");
  } catch (err) {
    console.error("removeUserFromProcedure error:", err);
    throw err;
  }
};

export const removeAllUsersFromProcedure = async (planId, procedureId) => {
  const url = `${api_url}/api/plan-procedures/${planId}/${procedureId}`;

  try {
    const response = await fetch(url, {
      method: "DELETE",
    });

    if (!response.ok)
      await handleError(response, "Failed to remove all users from procedure");
  } catch (err) {
    console.error("removeAllUsersFromProcedure error:", err);
    throw err;
  }
};
