export const categoriesApi = {
  getAll: async () => {
    const response = await fetch("/api/categories");
    return {
      ok: response.ok,
      data: response.ok ? await response.json() : null,
    };
  },

  updateAll: async (categories) => {
    const response = await fetch("/api/categories", {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(categories),
    });
    return {
      ok: response.ok,
      data: await response.json(),
    };
  },
};
