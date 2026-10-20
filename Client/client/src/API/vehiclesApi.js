export const vehiclesApi = {
  getAll: async (sortBy, descending) => {
    const response = await fetch(
      `/api/vehicles?sortBy=${sortBy}&descending=${descending}`,
    );
    return {
      ok: response.ok,
      data: response.ok ? await response.json() : null,
    };
  },

  create: async (newVehicle) => {
    const response = await fetch("/api/vehicles", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(newVehicle),
    });

    // Note: Even if response is not ok, we still parse the JSON to get validation errors
    return {
      ok: response.ok,
      data: await response.json(),
    };
  },
};
