const BASE_URL = '/api/tasks'

// Turns a failed response into a readable error message.
// For 400 responses, ASP.NET Core returns { errors: { Title: ["Title is required."] } }.
async function handleResponse(response) {
  if (response.ok) {
    return response.status === 204 ? null : response.json()
  }

  let message = `Request failed with status ${response.status}`
  try {
    const body = await response.json()
    if (body.errors) {
      message = Object.values(body.errors).flat().join(' ')
    } else if (body.title) {
      message = body.title
    }
  } catch {
    // Response had no JSON body; keep the default message.
  }
  throw new Error(message)
}

export async function getTasks() {
  return handleResponse(await fetch(BASE_URL))
}

export async function createTask(task) {
  return handleResponse(
    await fetch(BASE_URL, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(task),
    }),
  )
}

export async function updateTask(id, task) {
  return handleResponse(
    await fetch(`${BASE_URL}/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(task),
    }),
  )
}

export async function deleteTask(id) {
  return handleResponse(await fetch(`${BASE_URL}/${id}`, { method: 'DELETE' }))
}
